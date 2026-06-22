# Chats
Program for parsing Google chats from **Google Takeout**.

## Building the Program
* Requires Visual Studio + .NET 9

## Google Chat Messages
* These are chat messages from 2017 to present: https://en.wikipedia.org/wiki/Google_Chat
* To view this messages locally
  - Do a **Google Takeout** at https://takeout.google.com/ and make sure **Google Chat** is checked.
  - Extract the takeout.
  - In this program's **Setup** tab, set the **Google Chat Folder Path** to this folder within the takeout.

## Google Talk Messages
* These are chat messages from 2005 to 2017: https://en.wikipedia.org/wiki/Google_Talk
* To view these messages in **GMail** go to https://mail.google.com/mail/u/0/?ibxr=0#chats
* To view this messages locally
  - Do a **Google Takeout** at https://takeout.google.com/ and make sure **Mail** is checked.
  - Extract the takeout.
  - In this program run the utility **Extract Google Talk .eml from Google Takeout .mbox**.
    - This extracts all legacy chats from the **Google Takeout** .mbox.
  - In this program's **Setup** tab, set the **Google Talk Folder Path** to the folder you extracted the **Google Talk** .eml files to.

## Contacts
* To name and unify contacts (for example, if "alice@example.com" and "alice@aim" are the same person), create a custom .json file in this format.
* You can designate one of the contacts as yourself.
* If you don't know a person's real name you can use a screen name or alias as the name.
* Load the .json contact file into the program on the **Setup** tab.
```
[
    {
        "name": "Alice Person",
        "emails": ["alice.person@example.com"],
        "isSelf": true
    },
    {
        "name": "Bob Person",
        "emails": ["bp15ttt@aim", "bob.person@example.com", "bob.person@example.net"]
    },
    {
        "name": "\"persona\"",
        "emails": ["persona@aim"]
    },
    {
        "name": "\"personb\"",
        "emails": ["personb@aim"]
    }
]
```
