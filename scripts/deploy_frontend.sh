#!/bin/bash
set -e
cd ratemycourseload-frontend

BUILD_DIR="dist/"

echo "--- Building project"
npm run build
echo "--- Syncing files from build directory ($BUILD_DIR) to S3"
aws s3 sync --delete "$BUILD_DIR" "s3://ratemycourseload.com/"
echo "--- Done"
