# FitExporterLib
C# based library, export FIT file into data structure, or export into CSV/JSON

# How to use:
- Run command to show help:
```
CyclingAnalyzer.FitExporter.exe --help
```

# How to test with TestData:
- Copy the data locally. Say, in the folder: `c:\TestData\`. And create a sub-folder under `c:\TestData`: `c:\TestData\Q_1980_180_78_M`. And copy your FIT files into the new sub-folder.
- Run command:
```
CyclingAnalyzer.FitExporter.exe --input="c:\TestData" --output="c:\TestOutputData" --type=csv --format="{Name}_{BirthYear}_{Height}_{Weight}_{Gender}"
```
- Alternately, can copy the `c:\TestData\Q_1980_180_78_M` into the execution folder as `<EXECUTION_FOLDER>\TestData\Q_1980_180_78_M`, and run the command without input and output parameters:
```
CyclingAnalyzer.FitExporter.exe --format="{Name}_{BirthYear}_{Height}_{Weight}_{Gender}"
```