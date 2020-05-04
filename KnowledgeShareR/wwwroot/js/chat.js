"use strict";

const connection = new signalR.HubConnectionBuilder()
  .withUrl("/chatHub")
  .build();

const castVoteBtn = document.getElementById("castVoteBtn");
const userCountSpan = document.getElementById("user-count");
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
    } else {
      alert("Please select a valid answer");
    }

    event.preventDefault();
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

connection.on("ReceiveUserVote", function (user, message) {
  let msg = message
    .replace(/&/g, "&amp;")
    .replace(/</g, "&lt;")
    .replace(/>/g, "&gt;");
  let encodedMsg = "<span class='user-vote'>" + user + "</span>" + ": " + msg;
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

connection.on("NextQuestionReceived", function (question, answers) {
  if (question && answers) {
    if (voteQuestion && voteAnswers) {
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
          label: answer.substring(0, answer.indexOf(".") + 2),
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
    li.className = "list-group-item";
    li.innerText = answer;
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
    option.innerText = answer;
    voteAnswers.appendChild(option);
  });
}
