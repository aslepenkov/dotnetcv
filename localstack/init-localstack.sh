#!/bin/sh

# Step 1: Create IAM Role (Skip if Exists)
role_name="execution_role"
existing_role=$(awslocal iam get-role --role-name "$role_name" 2>/dev/null || echo "not_found")

if [ "$existing_role" = "not_found" ]; then
    echo "Creating IAM role: $role_name..."
    awslocal iam create-role \
        --role-name "$role_name" \
        --assume-role-policy-document '{"Version": "2012-10-17", "Statement": [{"Effect": "Allow", "Principal": {"Service": "lambda.amazonaws.com"}, "Action": "sts:AssumeRole"}]}'
else
    echo "IAM role '$role_name' already exists. Skipping creation."
fi

echo "Deploying .NET Lambda..."

# Get the pre-built Lambda package
LAMBDA_ZIP="/lambda/PostgresHealthLambda/src/PostgresHealthLambda/out/function.zip"

if [ ! -f "$LAMBDA_ZIP" ]; then
    echo "Error: Lambda package not found at $LAMBDA_ZIP"
    exit 1
fi

# Deploy the Lambda function
if awslocal lambda get-function --function-name "PostgresHealthLambda" > /dev/null 2>&1; then
    echo "Updating existing Lambda: PostgresHealthLambda"
    awslocal lambda update-function-code \
        --function-name "PostgresHealthLambda" \
        --zip-file "fileb://$LAMBDA_ZIP" \
        || { echo "Failed to update lambda"; exit 1; }
else
    echo "Creating new Lambda: PostgresHealthLambda"
    awslocal lambda create-function \
        --function-name "PostgresHealthLambda" \
        --runtime "dotnet8" \
        --role "arn:aws:iam::000000000000:role/execution_role" \
        --handler "PostgresHealthLambda::PostgresHealthLambda.Function::FunctionHandler" \
        --timeout 30 \
        --memory-size 256 \
        --zip-file "fileb://$LAMBDA_ZIP" \
        || { echo "Failed to create lambda"; exit 1; }
fi

echo "Lambda deployment complete!"

echo "Setting up SSM Parameter..."
awslocal ssm put-parameter \
    --name "/config/postgres/connection-string" \
    --value "Host=dotnetcv-postgres;Database=dotnetcv;Username=admin;Password=admin" \
    --type SecureString \
    --overwrite \
    || { echo "Failed to create ssm"; exit 1; }

echo "SSM Parameter Stored!"

# Create API Gateway REST API
API_ID=$(awslocal apigateway create-rest-api --name "PostgresHealthAPI" \
  --query 'id' --output text)

# Get the root resource ID
PARENT_ID=$(awslocal apigateway get-resources --rest-api-id "$API_ID" \
  --query 'items[0].id' --output text)

# Create a new resource: /postgres-health
RESOURCE_ID=$(awslocal apigateway create-resource --rest-api-id "$API_ID" \
  --parent-id "$PARENT_ID" --path-part "postgres-health" \
  --query 'id' --output text)

# Define a GET method for the resource
awslocal apigateway put-method --rest-api-id "$API_ID" \
  --resource-id "$RESOURCE_ID" --http-method GET --authorization-type "NONE"

# Integrate with Lambda (AWS_PROXY for direct invocation)
awslocal apigateway put-integration --rest-api-id "$API_ID" \
  --resource-id "$RESOURCE_ID" --http-method GET --type AWS_PROXY \
  --integration-http-method POST --uri \
  "arn:aws:apigateway:us-east-1:lambda:path/2015-03-31/functions/arn:aws:lambda:us-east-1:000000000000:function:PostgresHealthLambda/invocations"

# Grant API Gateway permission to invoke Lambda
awslocal lambda add-permission --function-name PostgresHealthLambda \
  --statement-id apigateway-test --action lambda:InvokeFunction \
  --principal apigateway.amazonaws.com --source-arn \
  "arn:aws:execute-api:us-east-1:000000000000:$API_ID/*/GET/postgres-health"

# Deploy the API
DEPLOYMENT_ID=$(awslocal apigateway create-deployment --rest-api-id "$API_ID" --stage-name "default" \
  --query 'id' --output text)

echo "API Gateway setup complete. API ID: $API_ID"
echo "API Endpoint: http://localhost:4566/restapis/$API_ID/default/_user_request_/postgres-health"
