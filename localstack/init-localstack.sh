#!/bin/sh

echo "Waiting for LocalStack to start..."
sleep 15  # Ensure LocalStack services are up

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


echo "Deploying .NET Lambdas..."

# Loop through all Lambda projects inside /lambda
for lambda_dir in /lambda/*/src/; do
  if [ -d "$lambda_dir" ]; then
    lambda_name=$(basename $(dirname "$lambda_dir"))  # Extract Lambda name from folder

    cd "$lambda_dir/$lambda_name/out" || { echo "Error: out folder not found"; exit 1; }

    if awslocal lambda get-function --function-name "$lambda_name" > /dev/null 2>&1; then
          echo "Updating existing Lambda: $lambda_name"
          awslocal lambda update-function-code \
              --function-name "$lambda_name" \
              --zip-file fileb://function.zip \
              || { echo "Failed to update lambda"; exit 1; }
    else
        echo "Creating new Lambda: $lambda_name"
        awslocal lambda create-function \
            --function-name "$lambda_name" \
            --runtime "dotnet8" \
            --role "arn:aws:iam::000000000000:role/execution_role" \
            --handler "PostgresHealthLambda::PostgresHealthLambda.Function::FunctionHandler" \
            --timeout 3 \
            --memory-size 128 \
            --zip-file fileb://function.zip \
            || { echo "Failed to create lambda"; exit 1; }
    fi

    echo "Lambda $lambda_name update complete!"
  fi
done

echo "All Lambdas deployed!"

# echo "Setting up SSM Parameter..."
# awslocal ssm put-parameter \
#     --name "/config/postgres/connection-string" \
#     --value "Host=dotnetcv-postgres;Database=dotnetcv;Username=admin;Password=admin" \
#     --type SecureString

# echo "SSM Parameter Stored!"