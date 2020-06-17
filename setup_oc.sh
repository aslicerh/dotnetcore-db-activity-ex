# Create a new OpenShift project
oc new-project demo

# Add the database
oc new-app postgresql-ephemeral

# Add the .NET Core application
oc new-app dotnet:3.1~https://github.com/aslicerh/dotnetcore-db-activity-ex --context-dir app

# Add envvars from the the postgresql secret, and database service name envvar.
oc set env deployment/dotnetcore-db-activity-ex --from=secret/postgresql -e database-service=postgresql

# Make the .NET Core application accessible externally and show the url
oc expose service dotnetcore-db-activity-ex
oc get route dotnetcore-db-activity-ex
