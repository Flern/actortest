#! /bin/bash

# Check minikube running
while ! minikube status | grep -q "host: Running" 2>&1 > /dev/null; do echo "Waiting for minikube ..." && sleep 5; done

# Check for dapr
if dapr status -k 2>&1 > /dev/null | grep "No status returned" 2>&1 > /dev/null; then
        echo "Initializing dapr ..."
        dapr init -k --wait
fi