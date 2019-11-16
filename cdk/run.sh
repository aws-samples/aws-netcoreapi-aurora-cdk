#!/bin/bash

dotnet build src
cdk bootstrap
cdk deploy --require-approval never