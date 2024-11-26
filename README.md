# PicheTestTask
This is a test assignment provided by Piche company.
 
# Build and run
1. Download or clone the repository
2. Make sure that .NET 8 SDK is installed and configured, so you can run `dotnet` commands from the terminal
3. Go to the repository folder
4. In the terminal, run `dotnet build -c Release`
5. Go to `\src\BankingApp.WebApi\bin\Release\net8.0`. You will find an executable file here (
 `BankingApp.WebApi.exe')
6. Run the executable file. YOu will see that the app is running on the default address `http://localhost:5000`
7. Open your browser and go to `http://localhost:5000/swagger` to see the API documentation.
8. You can send requests to the API from the Swagger page or in another way (like Postman)

# Notes
- The main task of this project is to show how I write code. I didn't want to spend too much time on it, so it lacks features like logging, integration tests, some unit tests and code best practices (like using viewmoels in responses to hide details such as database ID.)
- In production, I would have asked clarifying questions about exact requirements, but I didn't want to bother you with them, as it is only a test task.
- I wanted to show how I work with some logic anyway, so it became more complex that a simple 2-day task task (TBH I don't think 2 days is enough for such kind of tasks, otherwise than writing code, you need to spend some time on testing)
- The code uses ErrorOr library instead of exceptions, because exceptions are expensive performance-wise