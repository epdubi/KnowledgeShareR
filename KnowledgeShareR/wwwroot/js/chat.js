"use strict";

const connection = new signalR.HubConnectionBuilder().withUrl("/chatHub").build();

const castVoteBtn = document.getElementById("castVoteBtn");
const countDownBtn = document.getElementById("countDownButton");

if(castVoteBtn != undefined && countDownBtn != undefined)
{
    castVoteBtn.addEventListener("click", function (event) {
        const user = document.getElementById("username").innerText;
        const answerSelectList = document.getElementById("active-answers");
        const selectedAnswer = answerSelectList[answerSelectList.selectedIndex].value;
        console.log(user);
        console.log(selectedAnswer);

        if(selectedAnswer)
        {
            connection.invoke("SendUserVote", user, selectedAnswer).catch(function (err) {
                return console.error(err.toString());
            });
        }
        else
        {
            alert("Please select a valid answer");
        }
    
        event.preventDefault();
    });
    
    countDownBtn.addEventListener("click", function (event) {
        connection.invoke("CountDown").catch(function (err) {
            return console.error(err.toString());
        });
    
        event.preventDefault();
    });
}


connection.start().then(function () {
    console.log("Connection Started");
}).catch(function (err) {
    return console.error(err.toString());
});

connection.on("OnConnectedAsync", function (userList) {
    console.log(userList);

    //Clear User List
    let list = document.getElementById("usersList");
    let listUsers = Array.from(list.getElementsByTagName("li"));
    listUsers.map(x => x.remove());

    let parsedUsers = JSON.parse(userList);
    console.log(parsedUsers);
    parsedUsers.forEach(element => {
        let li = document.createElement("li");
        console.log(element);
        li.className = "badge badge-pill badge-primary";
        li.textContent = element;
        document.getElementById("usersList").appendChild(li); 
    });
});

connection.on("OnDisconnectedAsync", function (userList) {
    console.log(userList);

    //Clear User List
    let list = document.getElementById("usersList");
    let listUsers = Array.from(list.getElementsByTagName("li"));
    listUsers.map(x => x.remove());

    let parsedUsers = JSON.parse(userList);
    console.log(parsedUsers);
    parsedUsers.forEach(element => {
        let li = document.createElement("li");
        console.log(element);
        li.className = "badge badge-pill badge-primary";
        li.textContent = element;
        document.getElementById("usersList").appendChild(li); 
    });
});

connection.on("ReceiveUserVote", function (user, message) {
    let msg = message.replace(/&/g, "&amp;").replace(/</g, "&lt;").replace(/>/g, "&gt;");
    let encodedMsg = "<span class='user-vote'>" + user + "</span>" + ": " + msg;
    let li = document.createElement("li");
    li.innerHTML = encodedMsg;

    const messageList = document.getElementById("messagesList");
    if(messageList.getElementsByTagName("li")[0])
    {
        messageList.insertBefore(li, messageList.getElementsByTagName("li")[0]);
    }
    else
    {
        messageList.appendChild(li);
    }
});

connection.on("CountDownReceived", function (message) {
    let li = document.createElement("li");
    li.textContent = message;
    document.getElementById("messagesList").appendChild(li);
});
