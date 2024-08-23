# QuestionController
1. **[HttpPost]**  -- `api/question`: Create a question.
2. **[HttpPost]** -- `api/question/query`: Get all questions.
3. **[HttpGet]** /`api/question/{questionId}`: Get details of a specific question.
4. **[HttpPut]** -- `/api/question/{questionId}`: Update a specific question.
5. **[HttpDelete]** -- `/api/question/{questionId}`: Delete a specific question.
6. **[HttpPost]** -- `api/question/bulk-delete`: Delete multiple questions.

# ExamController
1. **[HttpPost]**  -- `/api/exam`: Create a new exam.
2. **[HttpPost]**  -- `/api/exam/query`: Get a list of all exams.
3. **[HttpGet]**  -- `/api/exam/{examId}`: Get details of a specific exam.
4. **[HttpPut]**  -- `/api/exam/{examId}`: Update a specific exam.
5. **[HttpDelete]**  -- `/api/exam/{examId}`: Delete a specific exam.
6. **[HttpPost]** -- `api/exam/bulk-delete`: Delete multiple exams.
7. **[HttpPost]**  -- `/api/exam/submit/{userId}`: Submit an exam.
8. **[HttpGet]**  -- `/api/exam/start/{examId}`: Start an exam.

# AuthController
1. **[HttpPost]**  -- `/api/auth/login`: Authenticate a user and provide a token.
2. **[HttpPost]**  -- `/api/auth/signup`
3. **[HttpPost]**  -- `/api/auth/confirm-account`
4. **[HttpPost]**  -- `/api/auth/resend-verification-code`
5. **[HttpPost]**  -- `/api/auth/logout`
6. **[HttpPost]**  -- `/api/auth/forget-password`
7. **[HttpPost]**  -- `/api/auth/reset-password`
8. **[HttpPost]**  -- `/api/auth/check-token`

# MemberController
1. **[HttpPost]** -- `/api/member/query`: Get all member list.
2. **[HttpGet]**  -- `/api/member/{userId/email}`: Get a specific member information by id.
3. **[HttpPut]**  -- `/api/member/{userId/email}`: Update member profile information.
4. **[HttpDelete]** -- `/api/member/{userId/email}`: Delete a member.

# ResultController
1. **[HttpGet]**  -- `/api/result/{resultId}`: Get a single exam result.

# LogController
1. **[HttpPost]** -- `/api/log/logs`: Get all the logs to view in tabulator.
2. **[HttpGet]** -- `/api/log/{id}`: Get Log  By id  to view in modal.
3. **[HttpDelete]** -- `/api/log/{id}`: Delete log.
4. **[HttpPost]** -- `/api/log/bulk-delete/{string[id]}`: Delete bulk of logs.
