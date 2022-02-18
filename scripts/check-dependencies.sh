#! /bin/bash

# Check for kubectl
if [ ! -f /usr/bin/kubectl ] && [ ! -f /usr/local/bin/kubectl ]; then
        echo "kubectl not detected. Installing now.  Requires 'sudo' privilege."
        sudo apt-get update
        sudo apt-get install -y apt-transport-https ca-certificates curl
        sudo curl -fsSLo /usr/share/keyrings/kubernetes-archive-keyring.gpg https://packages.cloud.google.com/apt/doc/apt-key.gpg
        echo "deb [signed-by=/usr/share/keyrings/kubernetes-archive-keyring.gpg] https://apt.kubernetes.io/ kubernetes-xenial main" | sudo tee /etc/apt/sources.list.d/kubernetes.list
        sudo apt-get update
        sudo apt-get install -y kubectl
fi

# Check for minikube
if [ ! -f /usr/local/bin/minikube ]; then
        echo "Minikube not detected.  Installing now.  Requires 'sudo' privilege."
        curl -LO https://storage.googleapis.com/minikube/releases/latest/minikube-linux-amd64
        sudo install minikube-linux-amd64 /usr/local/bin/minikube
fi

# Check for extensions
declare -a ext=( "ms-dotnettools.csharp" "ms-azuretools.vscode-dapr" "ms-azuretools.vscode-docker" "ms-kubernetes-tools.vscode-kubernetes-tools" )
extlist=$(code --list-extensions)
for i in "${ext[@]}"
do
        if [[ ! "$extlist" = *"$i"* ]]; then
                code --install-extension "$i"
        fi
done