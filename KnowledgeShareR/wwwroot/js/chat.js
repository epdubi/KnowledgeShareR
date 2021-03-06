﻿"use strict";

const connection = new signalR.HubConnectionBuilder()
  .withUrl("/knowledgeShareRHub")
  .configureLogging(signalR.LogLevel.Debug)
  .build();

const castVoteBtn = document.getElementById("castVoteBtn");
const privateMesssageBtn = document.getElementById("privateMessageBtn");
const groupMesssageBtn = document.getElementById("groupMessageBtn");
const hubGroupBtn = document.getElementById("hubGroupBtn");
const userCountSpan = document.getElementById("user-count");
const revealAnswerBtn = document.getElementById("reveal-answer");
const nextQuestionBtn = document.getElementById("next-question");
const answerList = document.getElementById("answer-list");
const voteQuestion = document.getElementById("vote-question");
const voteAnswers = document.getElementById("active-answers");

if (castVoteBtn != undefined) {
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

      let successDiv = document.querySelector("#question-options .alert");
      let successMessage = successDiv.querySelector("p");
      successDiv.removeAttribute("hidden");
      successMessage.innerHTML = selectedAnswer;
    } else {
      alert("Please select a valid answer");
    }

    event.preventDefault();
  });
}

if (privateMesssageBtn != undefined) {
  privateMesssageBtn.addEventListener("click", function (event) {
    event.preventDefault();
    var usersDropdown = document.getElementById("all-users");
    var selectedUser = usersDropdown[usersDropdown.selectedIndex].value;

    var privateMessage = document.getElementById("privateMessage").value;

    if (selectedUser && privateMessage) {
      connection
        .invoke("SendPrivateMessage", selectedUser, privateMessage)
        .catch(function (err) {
          return alert(err.toString());
        });
    } else {
      alert("Please select user and send message");
    }
    console.log(selectedUser);
    console.log(privateMessage);
  });
}

if (groupMesssageBtn != undefined) {
  groupMesssageBtn.addEventListener("click", function (event) {
    event.preventDefault();
    var groupsDropdown = document.getElementById("user-groups");
    var selectedGroup = groupsDropdown[groupsDropdown.selectedIndex].value;

    var groupMessage = document.getElementById("groupMessage").value;

    if (selectedGroup && groupMessage) {
      connection
        .invoke("SendGroupMessage", selectedGroup, groupMessage)
        .catch(function (err) {
          return alert(err.toString());
        });
    } else {
      alert("Please select group and send message");
    }
    console.log(selectedGroup);
    console.log(groupMessage);
  });
}

if (hubGroupBtn != undefined) {
  hubGroupBtn.addEventListener("click", function (event) {
    event.preventDefault();
    console.log("hubGroupBtn Clicked!");
    var hubGroupsDropdown = document.getElementById("hub-groups");
    var selectedGroup =
      hubGroupsDropdown[hubGroupsDropdown.selectedIndex].value;

    if (selectedGroup) {
      connection.invoke("AddToGroup", selectedGroup).catch(function (err) {
        return alert(err.toString());
      });
    } else {
      alert("Please select a group");
    }
    console.log(selectedGroup);
  });
}

if (revealAnswerBtn != undefined) {
  revealAnswerBtn.addEventListener("click", function (event) {
    event.preventDefault();

    let correctAnswerLi = document.querySelector(
      "li.list-group-item.correct-answer"
    );
    correctAnswerLi.classList.add("reveal");
  });
}

if (nextQuestionBtn != undefined) {
  nextQuestionBtn.addEventListener("click", function (event) {
    event.preventDefault();

    connection.invoke("NextQuestion").catch(function (err) {
      return alert(err.toString());
    });
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
  setUserCount(parsedUsers.length);
  buildUserList(parsedUsers);
});

connection.on("OnDisconnectedAsync", function (userList) {
  clearUserList();

  let parsedUsers = JSON.parse(userList);
  setUserCount(parsedUsers.length);
  buildUserList(parsedUsers);
});

connection.on("ReceiveUserVote", function (profilePicture, message) {
  let msg = message
    .replace(/&/g, "&amp;")
    .replace(/</g, "&lt;")
    .replace(/>/g, "&gt;");
  let encodedMsg =
    "<span class='user-vote'>" +
    "<img src='" +
    profilePicture +
    "' />" +
    "</span>" +
    " " +
    msg +
    "<hr/>";
  let li = document.createElement("li");
  li.innerHTML = encodedMsg;

  let messageList = document.getElementById("messagesList");
  if (messageList.getElementsByTagName("li")[0]) {
    messageList.insertBefore(li, messageList.getElementsByTagName("li")[0]);
  } else {
    messageList.appendChild(li);
  }

  let chatAnswer = msg.substring(0, msg.indexOf(".") + 2);
  let targetChartIndex = chart.options.data[0].dataPoints
    .map((e) => e.label)
    .indexOf(chatAnswer);

  var yValue = chart.options.data[0].dataPoints[targetChartIndex].y;
  if (yValue) {
    chart.options.data[0].dataPoints[targetChartIndex].y =
      chart.options.data[0].dataPoints[targetChartIndex].y + 1;
  } else {
    chart.options.data[0].dataPoints[targetChartIndex].y = 1;
  }
  chart.render();
});

connection.on("ReceivePrivateMessage", function (message) {
  alert(message);
});

connection.on("ReceiveGroupMessage", function (message) {
  alert(message);
});

connection.on("ReceiveGroupAdd", function (message) {
  alert(message);
});

connection.on("NextQuestionReceived", function (question, answers) {
  if (question && answers) {
    if (voteQuestion && voteAnswers) {
      let successDiv = document.querySelector("#question-options .alert");
      let successMessage = successDiv.querySelector("p");
      successDiv.hidden = true;
      successMessage.innerHTML = "";

      voteQuestion.innerHTML = question;
      clearVoteAnswers();
      buildVoteAnswers(answers);
    }

    clearAnswerList();
    buildAnswersList(answers);

    if (chart) {
      var newDataPoints = [];
      answers.map((answer, i) =>
        newDataPoints.push({
          label: answer.answer.substring(0, answer.answer.indexOf(".") + 2),
          x: i,
        })
      );

      chart = new CanvasJS.Chart("chartContainer", {
        animationEnabled: true,
        theme: "light2",
        title: {
          text: question,
        },
        axisY: {
          title: "Votes",
        },
        data: [
          {
            type: "column",
            showInLegend: true,
            legendMarkerColor: "grey",
            legendText: "Users",
            dataPoints: newDataPoints,
          },
        ],
      });
      chart.render();
    }
  }
});

function clearUserList() {
  let list = document.getElementById("usersList");
  let listUsers = Array.from(list.getElementsByTagName("li"));
  listUsers.map((x) => x.remove());
}

function buildUserList(parsedUsers) {
  parsedUsers.forEach((element) => {
    let li = document.createElement("li");
    li.className = "badge badge-pill badge-primary";
    li.innerText = element;
    document.getElementById("usersList").appendChild(li);
  });
}

function setUserCount(userCount) {
  let userCountText = "(" + userCount + ")";
  userCountSpan.innerText = userCountText;
}

function clearAnswerList() {
  let answerListItems = Array.from(answerList.getElementsByTagName("li"));
  answerListItems.map((x) => x.remove());
}

function buildAnswersList(answers) {
  answers.forEach((answer) => {
    let li = document.createElement("li");
    li.className = answer.isCorrect
      ? "list-group-item correct-answer"
      : "list-group-item";
    li.innerText = answer.answer;
    answerList.appendChild(li);
  });
}

function clearVoteAnswers() {
  document
    .querySelectorAll("#active-answers option")
    .forEach((option) => option.remove());
}

function buildVoteAnswers(answers) {
  let titleOption = document.createElement("option");
  titleOption.innerText = "Select Answer";
  voteAnswers.appendChild(titleOption);

  answers.forEach((answer) => {
    let option = document.createElement("option");
    option.innerText = answer.answer;
    voteAnswers.appendChild(option);
  });
}
