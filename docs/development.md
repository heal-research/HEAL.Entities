# Development and contribution

## Prerequisites

1. git repository
1. install .NET Core >= 2.x
1. install [Nerdbank.GitVersioning .NET Core CLI tool](https://github.com/dotnet/Nerdbank.GitVersioning).  
    - only necessary if you want to contribute and create *release candidate* or *release* branches
    - for more information on versioning read [Versioning and deployment](#versioning-and-deployment)

## Building the project
Is as easy as:
- run `build solution` in visual studio 
- or run `BUILD.cmd`

## Versioning and deployment
This project utilizes the [Nerdbank.GitVersioning](https://github.com/dotnet/Nerdbank.GitVersioning) package for stamping assemblies and version assignment and follows the [semantic versioning guideline](https://semver.org/#spec-item-9). 

```
1.0.0-prerelease < 1.0.1-prerelease < 1.0.1-rc < 1.0.1
```

The master and feature branches are used for feature development and track versions with the `prerelease` suffix. These branches are considered by the CI pipeline which regularly builds publicly available packages and pushes them to the [public feed]( ). 