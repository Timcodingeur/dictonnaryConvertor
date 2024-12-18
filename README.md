# Dictionary Convertor

Welcome to the **Dictionary Convertor**! This project is aimed at converting dictionaries from various formats into a structured JSON format. Currently, we are focusing on a Romansh-German dictionary for testing purposes, with plans to extend to more formats and languages in the future.
## Langages and Frameworks


![MySQL](https://img.shields.io/badge/mysql-%2300f.svg?style=for-the-badge&logo=mysql&logoColor=white)
![Visual Studio](https://img.shields.io/badge/Visual%20Studio-5C2D91.svg?style=for-the-badge&logo=visual-studio&logoColor=white)
![Docker](https://img.shields.io/badge/docker-%230db7ed.svg?style=for-the-badge&logo=docker&logoColor=white)
![Git](https://img.shields.io/badge/git-%23F05033.svg?style=for-the-badge&logo=git&logoColor=white)
![GitHub](https://img.shields.io/badge/github-%23121011.svg?style=for-the-badge&logo=github&logoColor=white)
![ChatGPT](https://img.shields.io/badge/chatGPT-74aa9c?style=for-the-badge&logo=openai&logoColor=white)

## Features
- Extract text from PDF dictionaries and convert them to JSON.
- Uses regex to parse dictionary entries effectively.
- Preserves original content as accurately as possible for ease of use in other applications.

## Getting Started

### Prerequisites
- **Visual Studio 2022** or later (required to run the project).
- **.NET 8.0 SDK** (make sure it's installed).
- **NuGet Packages**: Install necessary dependencies including `UglyToad.PdfPig` for PDF extraction.

### Installation

1. Clone the repository:
   ```bash
   git clone https://github.com/Timcodingeur/dictonnaryConvertor.git
   ```

2. Navigate to the project folder:
   ```bash
   cd dictonnaryConvertor
   cd dictionnaryCreator
   ```

### Running the Application

Since the project is still in development, there are no executables yet. You need to run it directly through **Visual Studio**:

1. Open `dictonnaryConvertor.sln` in Visual Studio.
2. Set `dictionnaryCreator` as the startup project.
3. Click on **Run** (F5) to execute the program.

### Output Location

After running, the output JSON file (`dictionary.json`) and the original dictionary (`Dizionari_dels_idioms_romauntschs_d_Engi.pdf`) can be found in:

```
dictionnaryCreator\bin\Debug\net8.0
```

## Future Plans
- Add support for more dictionary formats like `.txt`, `.xml`, and other common file types.
- Generate executables to make the tool easier to run without Visual Studio.
- Improve the parser for greater accuracy and versatility across different dictionary styles.

## Contributions
Contributions are welcome! Feel free to fork the repository, make your changes, and submit a pull request.

## License
This project is open source and available under the [MIT License](LICENSE).


