#!/bin/bash

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
awslocal apigateway create-deployment --rest-api-id "$API_ID" --stage-name dev

echo "API Gateway setup complete. API ID: $API_ID"
