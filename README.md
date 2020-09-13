# CognitiveServicesCLI
A Command line interface way of interacting with Azure Cognitive Services

## Usage
dotnet run -- --location WestUS -k {api-key} -ta -sa -txt "Sunday has been a lovely day of relaxation. I managed to plant some vegetables and herbs before the rain" -ka

The above syntax is:
* --location = location of the Azure Cognitive service
* -k = API Key of the Azure service
* -ta = Use text analytics series of options (currently only one supported with -cv Computer vision coming next)
* -sa = Peform sentiment analysis
* -txt = The text to analyse
* -ka = Perform keyphrase analysis
