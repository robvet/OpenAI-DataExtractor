# Azure OpenAI App for Text Extraction

This app accepts a single .pdf file along with an OpenAI prompt and then parses through the .pdf file to extract the text elements or directives specificed in the prompt.

It also contains a ChatBot App that leverages the Azure OpenAI Completions API to go against a ChatGPT model.

Items of Interest Include: 

- For each chatturn, the ChatBot makes a second call to the model passing in the completion asking for `three follow-up suggestions`. The suggestions are rendered in the ChatBot with simple hyperlinks to invoke them.
- The `TokenManager` service implements a slideing decay window that maintains the most recent chat history, but summarizes earlier history to stay within the required token limits.
- The UX is built with `Blazor 8, Interactvie Auto` components. The Blazor Team has evolved the product. Am looking forward to more documentation and examples on how to use this new version. Unfortunately, spent way too much time figuring out how to make UX logic work.