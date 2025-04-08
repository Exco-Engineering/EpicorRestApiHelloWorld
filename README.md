# EpicorRestApiHelloWorld
Reference project showing how to use Epicor's REST API

## API key and user name
This project requires the epicor credentials to be passed in as enviroment variables<br>

We want to keep these out of the repo and user's computers.<br>

Ideally, only your server holds these (as database entry or enviroment variables), and the clients just ask the server for data. Ask Rodrigo for more details.

## Docs (Official Epicor Docs)
All you need to figure the rest API out is in the official documentation page epicor provides here:<br>

https://ee-erp11-app.excoeng.com/ERP11-PRD/api/help/v2/index.html<br>
https://ee-erp11-test.excoeng.com/ERP11-TST/api/help/v2/index.html<br>

To access this you need a Exco Epicor Username and Password<br>

## Example 
See code and docs for notes about getting data from Epicor "normal" services or BAQ "custom" services.
![image](https://github.com/user-attachments/assets/18310231-de6f-4d71-ab6d-4c8ea6c31deb)

## Dependencies
Uses the Newtonsoft.Json library for json parsing. <br>
As much as I like reducing dependencies, writing a json parser is not my idea of fun..... <br>
It's entered in the project as a Nuget package so it should be seamless to build, if not let me know.<br>
![image](https://github.com/user-attachments/assets/74d131ac-719e-45c0-b25b-d23950446652)
