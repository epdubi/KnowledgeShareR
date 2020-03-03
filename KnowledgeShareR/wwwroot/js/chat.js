"use strict";

const connection = new signalR.HubConnectionBuilder().withUrl("/chatHub").build();

const sendBtn = document.getElementById("sendButton");
const countDownBtn = document.getElementById("countDownButton");
const userForm = document.getElementById("usernameInput");

if(sendBtn != undefined && countDownBtn != undefined)
{
    sendBtn.addEventListener("click", function (event) {
        const user = document.getElementById("username").innerText;
        const message = document.getElementById("messageInput").value;
        console.log(user);
        connection.invoke("SendMessage", user, message).catch(function (err) {
            return console.error(err.toString());
        });
    
        event.preventDefault();
    });
    
    countDownBtn.addEventListener("click", function (event) {
        connection.invoke("CountDown").catch(function (err) {
            return console.error(err.toString());
        });
    
        event.preventDefault();
    });
}

if(userForm != undefined)
{
    userForm.addEventListener("submit", function (event) {
        const username = document.getElementById("UserName").value;
        if(username)
        {
            connection.invoke("ConnectUser", username).catch(function (err) {
                return console.error(err.toString());
            });
        }
    });
}

connection.start().then(function () {
    console.log("Connection Started");
}).catch(function (err) {
    return console.error(err.toString());
});

connection.on("OnConnectedAsync", function (message) {
    console.log(message);
});

connection.on("OnDisconnectedAsync", function (message) {
    let usernameCookie = document.cookie;
    if(usernameCookie)
    {
        usernameCookie = usernameCookie + '=; expires=Thu, 01 Jan 1970 00:00:01 GMT;';
    }
});

connection.on("ReceiveMessage", function (user, message) {
    let msg = message.replace(/&/g, "&amp;").replace(/</g, "&lt;").replace(/>/g, "&gt;");
    let encodedMsg = user + " says " + msg;
    let li = document.createElement("li");
    li.textContent = encodedMsg;
    document.getElementById("messagesList").appendChild(li);
});

connection.on("CountDownReceived", function (message) {
    let li = document.createElement("li");
    li.textContent = message;
    document.getElementById("messagesList").appendChild(li);
});

connection.on("UserConnected", function (userList) {
    if(userList)
    {
        //Clear User List
        let list = document.getElementById("usersList");
        let listUsers = Array.from(list.getElementsByTagName("li"));
        listUsers.map(x => x.remove());

        let parsedUsers = JSON.parse(userList);
        console.log(parsedUsers);
        parsedUsers.forEach(element => {
            let li = document.createElement("li");
            li.textContent = element;
            document.getElementById("usersList").appendChild(li); 
        });
    }
});

