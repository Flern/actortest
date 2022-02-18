#! /bin/bash

# Check for dapr install
if [ ! -f /usr/local/bin/dapr ]; then
        echo "Dapr CLI not detected.  Installing now.  Requires 'sudo' privilege."
        sudo wget -q https://raw.githubusercontent.com/dapr/cli/master/install/install.sh -O - | /bin/bash
fi

# Check for dapr
if dapr status -k 2>&1 > /dev/null | grep "No status returned" 2>&1 > /dev/null; then
        echo "Initializing dapr ..."
        dapr init -k --wait
fi