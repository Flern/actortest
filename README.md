# actortest

Repository for reproducing an apparent breakpoint issue in VSCode Kubernetes remote attach debugging.
<br><br>

# Dependencies

<p>This code has been executed in WSL2 on a Windows 10 PC with Ubuntu 20.04.  The functionality should be compaitble with
any Linux distro that has Docker installed.</p>

**Running the VSCode task `check-dependencies` will attempt to install all necessary dependencies**
<p>

## Dependencies for the full demonstration include:
 - VSCode extensions
   - ms-dotnettools.csharp
   - ms-azuretools.vscode-dapr
   - ms-azuretools.vscode-docker
   - ms-kubernetes-tools.vscode-kubernetes-tools
 - dapr
 - minikube
 - kubectl

 # Demonstration

 ## Setup
  1. Build the code using the `build` task.
  2. Set breakpoints on:
     - line 73 of `Program.cs`
     - line 10 of `src/AbstractExample/AbstractExampleActor.cs`
     - line 10 of `src/AbstractExample/AbstractExample.cs`
     - line 26 of `src/Application/AggregateActor.cs`

## Exercising the demo
The demonstration project includes an HTTP endpoint that accepts a boolean value (`true` or `false`) to drive operation
of the dapr actor in a test setup or through the dapr infrastructure itself.  The `dotnet run` environment does not
include the dapr runtime, so only the `true` value will result in successful execution.

 1. Start the target environment
 2. Start VSCode debugger by launching or attaching to the actortest binary
 3. Open the web browser to the proper port with the `true` or `false` router parameter binding indicated (e.g., http://localhost:5000/true)

# Environment-specific operation
The breakpoint will be successfully hit in two of the three methods for running this example.

## Command line `dotnet run`
Using the VSCode debugger, launch the **.NET Core Launch (console)** profile.  Open http://localhost:5000/true and
observe the breakpoints being hit.  All breakpoints will successfully be matched to code files.

## Command line `dapr`
Using the VSCode debugger, launch the **.NET with Dapr** profile.  Open http://localhost:5000/true and
observe the breakpoints being hit.  All breakpoints will successfully be matched to code files.  Repeat with
http://localhost:5000/false (to invoke the actor runtime) and observe the same success.

## Kubernetes
For kubernetes, we need to start the environment, perform a docker build, apply a YAML file, attach the debugger, and
forward ports to localhost to demonstrate the issue.

1. First start the minikube environment by running the `start-kube-environment` task and wait for the cluster to start and
dapr initialization to complete.
2. Now execute the `docker build` build task in VSCode.  This build will target the minikube docker daemon for easy deployment.
3. Once the docker build completes, open the `deploy/actortest.yml` file and run the **Kubernetes: Apply** command from
   the command palette.  Since the YAML file contains all the resources necessary for the deployment, you'll get a popup
   warning about VSCode not being able to indicate what will change.  Click Apply - we'll be fine.
4. Run the **Kubernetes: Debug (Attach)** command from the command palette and select the `actortest-<id>` pod,
   `actortest` container, and the `dotnet` environment.  You should have an active debug session.
5. Finally, use the `forward-ports` command to launch a terminal process that fowards localhost port 5000 to port 80 on
   the actortest service.  This allows the same access method as the other two environments.

### Proper operation of breakpoints
Open http://localhost:5000/true to exercise the test path in the code.  All breakpoints set will resolve to source code
locations as before.

### Missing breakpoing / source mapping
Now steer your browser to http://localhost:5000/false to exercise the actor runtime.  Three of the four breakpoints hit
as before and resolve to source code mapping.  The fourth breakpoint (at line 10 of
`src/AbstractExample/AbstractExample.cs`) breaks the execution but provides no mapping to source code in the IDE.  Any
entry into the debug console results in an *Evaluation failed* indication.  The call stack indicates a .NET ThreadPool
Worker is `PAUSED ON BREAKPOINT`, but the stack trace displays *Error processing 'stackTrace' request. Unknown Error: 0x80131c35*

## Kubernetes cleanup
Disconnect the debug session and run the `cleanup-kube-environment` task.  This will remove dapr from the cluster and
stop minikube.
