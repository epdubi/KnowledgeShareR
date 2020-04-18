"use strict";

const connection = new signalR.HubConnectionBuilder()
  .withUrl("/chatHub")
  .build();

const castVoteBtn = document.getElementById("castVoteBtn");
const countDownBtn = document.getElementById("countDownButton");

if (castVoteBtn != undefined && countDownBtn != undefined) {
  castVoteBtn.addEventListener("click", function (event) {
    let user = document.getElementById("username").innerText;
    let answerSelectList = document.getElementById("active-answers");
    let selectedAnswer = answerSelectList[answerSelectList.selectedIndex].value;

    if (selectedAnswer) {
      connection
        .invoke("SendUserVote", user, selectedAnswer)
        .catch(function (err) {
          return alert(err.toString());
        });
    } else {
      alert("Please select a valid answer");
    }

    event.preventDefault();
  });

  countDownBtn.addEventListener("click", function (event) {
    connection.invoke("CountDown").catch(function (err) {
      return alert(err.toString());
    });

    event.preventDefault();
  });
}

connection
  .start()
  .then(function () {
    console.log("Connection Started");
  })
  .catch(function (err) {
    return console.error(err.toString());
  });

connection.on("OnConnectedAsync", function (userList) {
  clearUserList();

  let parsedUsers = JSON.parse(userList);
  parsedUsers.forEach((element) => {
    let li = document.createElement("li");
    li.className = "badge badge-pill badge-primary";
    li.textContent = element;
    document.getElementById("usersList").appendChild(li);
  });
});

connection.on("OnDisconnectedAsync", function (userList) {
  clearUserList();

  let parsedUsers = JSON.parse(userList);
  parsedUsers.forEach((element) => {
    let li = document.createElement("li");
    li.className = "badge badge-pill badge-primary";
    li.textContent = element;
    document.getElementById("usersList").appendChild(li);
  });
});

connection.on("ReceiveUserVote", function (user, message) {
  let msg = message
    .replace(/&/g, "&amp;")
    .replace(/</g, "&lt;")
    .replace(/>/g, "&gt;");
  console.log(msg);
  let encodedMsg = "<span class='user-vote'>" + user + "</span>" + ": " + msg;
  let li = document.createElement("li");
  li.innerHTML = encodedMsg;

  let messageList = document.getElementById("messagesList");
  if (messageList.getElementsByTagName("li")[0]) {
    messageList.insertBefore(li, messageList.getElementsByTagName("li")[0]);
  } else {
    messageList.appendChild(li);
  }

  let targetChartIndex = chart.options.data[0].dataPoints
    .map((e) => e.label)
    .indexOf(msg);
  console.log(targetChartIndex);

  var yValue = chart.options.data[0].dataPoints[targetChartIndex].y;
  if (yValue) {
    chart.options.data[0].dataPoints[targetChartIndex].y =
      chart.options.data[0].dataPoints[targetChartIndex].y + 1;
  } else {
    chart.options.data[0].dataPoints[targetChartIndex].y = 1;
  }
  chart.render();
});

connection.on("CountDownReceived", function (message) {
  let li = document.createElement("li");
  li.textContent = message;
  document.getElementById("messagesList").appendChild(li);
});

function clearUserList() {
  let list = document.getElementById("usersList");
  let listUsers = Array.from(list.getElementsByTagName("li"));
  listUsers.map((x) => x.remove());
}
