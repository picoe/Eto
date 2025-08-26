Eto.Forms uses the following versioning scheme:

1. The master branch is the stable/release branch.
2. The develop branch is used for ongoing development, merging, etc.
3. The develop branch uses an auto generated build/revision based off the build date/time.

### v2.0 stable, 2.1 in development

Branch       | Version      | File Version | Informational/Nuget
------------ | ------------ | ------------ | -------------------
master-2.0   |  2.0.0.0     | 2.0.0.0      | 2.0.0
develop-2.1  |  2.0.*       | 2.0.*        | 2.1.0-build{build}


### v2.1 stable, 2.2 in development

Branch       | Version      | File Version | Informational/Nuget
------------ | ------------ | ------------ | -------------------
master-2.1   |  2.1.0.0     | 2.1.0.0      | 2.1.0         
develop-2.2  |  2.1.*       | 2.1.*        | 2.2.0-build{build}


## Patches

Patches will be based off the master branch and increment the nuget patch number and file version, but retain the same assembly version.  This allows for strong naming compatible assemblies. For example, a new patch to 2.0 would result in:

Branch       | Version      | File Version | Informational/Nuget
------------ | ------------ | ------------ | -------------------
master-2.0.1 |  2.0.0.0     | 2.0.1.0      | 2.0.1
develop-2.1  |  2.0.*       | 2.0.*        | 2.1.0-alpha{build}
