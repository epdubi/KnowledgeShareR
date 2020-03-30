SELECT * FROM Questions q 
JOIN Answers a ON q.QuestionId = a.QuestionId
JOIN Votes v ON q.QuestionId = v.QuestionId

INSERT INTO Questions(Text, IsActive)
VALUES('What is your favorite color?', 1)

INSERT INTO Answers(QuestionId, Text, IsCorrect)
VALUES(1, 'Blue', 0)

INSERT INTO Answers(QuestionId, Text, IsCorrect)
VALUES(1, 'Green', 1)

INSERT INTO Answers(QuestionId, Text, IsCorrect)
VALUES(1, 'Pink', 0)

INSERT INTO Answers(QuestionId, Text, IsCorrect)
VALUES(1, 'Orange', 0)