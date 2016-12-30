# Contributing

Here's a quick guide.

Clone the repo:

    git clone http://gitlab.santiagodevelopment.com/jamessantiago/getnet.git

Alternatively, visual studio can be used to download the repo.  Go to file -> open -> open from source control -> connect -> local git repositories -> clone.

Create a new branch:

    git branch -b MyDevBranch

Make sure the tests pass:

Note: many tests have environment requirements such as access to an ldap source.  Another note: need to configure tests to pull from corecurrent config (which prioritizes environment variables) instead of hardcoding config data.

 - Add unit tests for any new functionality
 - Build the `getnet.tests` project
 - Open up the test explorer and run all tests

Ensure you merge in the latest master commits:

    git pull origin master

Push to your branch and [submit a pull request][pr]:

   git push origin MyDevBranch

[pr]: http://gitlab.santiagodevelopment.com/jamessantiago/getnet/compare

## Setting up a dev environment

*  Install Windows 10 x64
*  Install visual studio 2015 Update 3
*  Install visual studio 2015 tool preview 2015
*  Install .NET core 1.1 SDK and runntime (https://www.microsoft.com/net/download/core)
*  Install the "Bundler and Minifier" visual studio extension

Optionally, GNS3 can be installed to do the SSH tests, MS SQL or Postgres can be installed to do the entity framework tests, and ldap on linux or windows can be installed to do the ldap tests..  The GNS3 project template and router configs are included in the source files.  Git, cmder, and OpenSSH via cygwin area also recommended.  Web essentials as a visual studio addon may also be useful.

