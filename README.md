# EFIssueSimulator

This repository was created to simulate the following issue on Entity Framework 6.1.3.
Issue link: https://github.com/aspnet/EntityFramework6/issues/31

# Before run tests

To run, you must clone EF repo first, and it should be in parent folder from this repository, like this:

````
C:\SOME_FOLDER
├───EfIssueSimulator
├───EntityFramework6
````

# Runing

There is 2 tests in project EfIssueSimulator.Test

The test ThereIsNoIssueWhenInsertARecordWithoutIDbCommandTreeInterceptor just run a simple insert with EF without using the Interceptors.

The test **ThereIsIssueWhenInsertARecordWithIDbCommandTreeInterceptor** reproduce the issue, adding the **MultiTenantTreeInterceptor** and **MultiTenantInterceptor**
