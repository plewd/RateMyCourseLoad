#!/bin/bash
set -e
cd ratemycourseload-backend

echo "--- Deploying [you will have to confirm]"
sam build && sam deploy
echo "--- Done"
