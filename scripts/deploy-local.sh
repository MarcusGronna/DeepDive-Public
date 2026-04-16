#!/usr/bin/env bash
set -e

IMAGE_NAME="infrasprint-api:dev"
CLUSTER_NAME="infrasprint"
DEPLOYMENT_NAME="infrasprint-api"

echo "Building Docker image..."
docker build -t $IMAGE_NAME .

echo "Loading image into kind..."
kind load docker-image $IMAGE_NAME --name $CLUSTER_NAME

echo "Applying Kubernetes manifest..."
kubectl apply -f k8s/app.yaml

echo "Restarting deployment to force new pod..."
kubectl rollout restart deployment/$DEPLOYMENT_NAME

echo "Waiting for rollout..."
kubectl rollout status deployment/infrasprint-api

echo "Done."
