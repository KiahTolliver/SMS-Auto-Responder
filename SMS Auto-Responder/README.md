# SMS Auto Responder
A simple ASP.NET Core application demonstrating the ability to send and receive SMS messages using the Telnyx Messaging API

## Pre-requisites
* a [Telnyx Account](https://telnyx.com/sign-up)
* a [Telnyx Phone Number](https://portal.telnyx.com/#/app/numbers/my-numbers)
* [ngrok](https://ngrok.com/) or a similar service that allows you to receive public webhook requests to your local development environment
* [DotNet Core](https://developers.telnyx.com/docs/v2/development/dev-env-setup?lang=java&utm_source=referral&utm_medium=github_referral&utm_campaign=cross-site-link) installed

## Setup
### Step 1 - Cloning the repo and installing dependencies

$ git clone


### Step 2 - .env
This application uses [dotenv](https://github.com/motdotla/dotenv) package to manage environment variables. 
Make a copy of [`.env.sample`](./.env.sample), save it as `.env`, and update the variables to match your creds.
| Variable               | Description                                                                                                                                              |
|:-----------------------|:---------------------------------------------------------------------------------------------------------------------------------------------------------|
| `TELNYX_API_KEY`       | Your [Telnyx API Key](https://portal.telnyx.com/#/app/api-keys)              |
| `TELNYX_PUBLIC_KEY`    | Your [Telnyx Public Key](https://portal.telnyx.com/#/app/account/public-key) |
| `TELNYX_APP_PORT`      | **Defaults to `8000`** The port the app will be served                           


### Step 3 - ngrok

This application is served on the port defined in the runtime environment (or in the `.env` file). Be sure to launch [ngrok](https://developers.telnyx.com/docs/v2/development/ngrok?utm_source=referral&utm_medium=github_referral&utm_campaign=cross-site-link) for that port

```
./ngrok http 8000
```

> Terminal should look similar to the snippet below

```
ngrok by @inconshreveable                                                                                                                               (Ctrl+C to quit)

Session Status                online
Account                       Kiah Tolliver (Plan: Free)   
Version                       2.3.38
Region                        United States (us)
Web Interface                 http://127.0.0.1:4040
Forwarding                    http://your-url.ngrok.io -> http://localhost:8000
Forwarding                    https://your-url.ngrok.io -> http://localhost:8000

Connections                   ttl     opn     rt1     rt5     p50     p90
                              116     0       0.00    0.00    4.98    196.12
```

At this point you can point your application to generated ngrok URL + path  (Example: `http://{your-url}.ngrok.io/messaging/inbound`).

### Step 4 - Set up a Telnyx messaging profile
Assuming you have a [Telnyx account](https://telnyx.com/sign-up) and have set up [a number](https://portal.telnyx.com/#/app/numbers/my-numbers), access the [Messaging Portal](https://portal.telnyx.com/#/app/messaging) and "Add new profile". You should be taken to a page that looks as follows:

Give the profile a name in the **Profile Name** field. This example uses the name "SMS Auto Responder". 

![Add new profile](images/add_profile.png)

Then, enter the application url from the previous step in the **Send a webhook to this URL** field, adding `/messaging/inbound` to the end of it. This is done because the application receives inbound SMS webhooks through this endpoint, as defined in MessagingController.cs. The complete url should look something like `https://{your-url}.ngrok.io/messaging/inbound` if you are using ngrok.

Finally, navigate to the [Numbers Portal](https://portal.telnyx.com/#/app/numbers/my-numbers) and assign the previously created messaging profile to your Telnyx number.

![Attach messaging profile to phone number](images/set_profile.png)

This example attaches the "SMS Auto Responder" messaging profile to the number +1-800-000-0000.

### Step 5 - Run the Application

Open your IDE and run the application

### Special Mentions:
* Telnyx's [asp.net-messaging](https://github.com/team-telnyx/demo-dotnet-telnyx/tree/master/asp.net-messaging) repo for code samples and README guidelines
* Telnyx's [.NET API Docs](https://developers.telnyx.com/docs/v2/development/dev-env-setup?lang=net)