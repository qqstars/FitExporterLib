# FitExporterLib
C# based library, export FIT file into data structure, or export into CSV/JSON

# How to use:
- Run command to show help:
```
CyclingAnalyzer.FitExporter.exe --help
```

# How to test with TestData:
- Copy the data locally. Say, in the folder: `c:\TestData\`
- Run command:
```
CyclingAnalyzer.FitExporter.exe --input="c:\TestData" --output="c:\TestOutputData" --type=csv --format="{Name}_{BirthYear}_{Height}_{Weight}_{Gender}"
```
- Alternately, can copy the `TestData` into the execution folder, and run the command without input and output parameters:
```
CyclingAnalyzer.FitExporter.exe --format="{Name}_{BirthYear}_{Height}_{Weight}_{Gender}"
```