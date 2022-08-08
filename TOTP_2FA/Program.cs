using OtpNet;

string base32Secret = "6L4OH6DDC4PLNQBA5422GM67KXRDIQQP";
int step = (60); // step time to generate new code on seconds
HashSet<long>? usedTimeSteps = new HashSet<long>();
Menu();

void Menu()
{
    Console.WriteLine("Select enter one option:");
    Console.WriteLine("1 -> Generate token");
    Console.WriteLine("2 -> Verify token One-time strict");
    Console.WriteLine("3 -> Verify token");
    Console.WriteLine("4 -> Generate random base key");
    Console.WriteLine("5 -> Change step time to generate new codes");
    Console.WriteLine("6 -> Set key");
    Console.WriteLine("     Close program to exit: ");
    string inputOption = Console.ReadLine();
    string? inputCode;
    bool valid;
    switch (inputOption)
    {
        case "1":
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"Token : {GenerateTOTP()}");
            Console.ResetColor();
            Menu();
            break;
        case "2":
            Console.Write("Enter token: ");
            inputCode = Console.ReadLine();
            valid = VerifyTOTPStrict(inputCode ?? "");
            Console.ForegroundColor = valid ? ConsoleColor.Green : ConsoleColor.Red;
            Console.WriteLine($"{(valid ? "Valid" : "Invalid")} | {DateTime.Now}");
            Console.ResetColor();
            Menu();
            break;
        case "3":
            Console.Write("Enter token: ");
            inputCode = Console.ReadLine();
            valid = VerifyBaseTOTP(inputCode ?? "", out _);
            Console.ForegroundColor = valid ? ConsoleColor.Green : ConsoleColor.Red;
            Console.WriteLine($"{(valid ? "Valid" : "Invalid")} | {DateTime.Now}");
            Console.ResetColor();
            Menu();
            break;
        case "4":
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine($"Configure 2FA Key: {GenerateRandomBaseKey()}");
            Console.ResetColor();
            Menu();
            break;
        case "5":
            try
            {
                Console.Write("Enter time on seconds: ");
                step = int.Parse(Console.ReadLine() ?? "30");
                Console.ForegroundColor = ConsoleColor.DarkGray;
                Console.WriteLine($"Step configured: {step}");
                Console.ResetColor();
            }
            catch (Exception)
            {
                Console.ForegroundColor = ConsoleColor.DarkRed;
                Console.WriteLine($"Only accept integer number");
                Console.ResetColor();
            }
            finally
            {
                Menu();
            }
            break;
        case "6":
            Console.Write("Enter key: ");
            base32Secret = Console.ReadLine();
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.WriteLine($"Key configured: {base32Secret}");
            Console.ResetColor();
            Menu();
            break;
        default:
            Menu();
            break;
    }
}

string GenerateRandomBaseKey() {
    var secret = KeyGeneration.GenerateRandomKey(20);
    base32Secret = Base32Encoding.ToString(secret);
    return base32Secret;
}

string GenerateTOTP() {
    var secret = Base32Encoding.ToBytes(base32Secret);
    var totp = new Totp(secret, step);
    var code = totp.ComputeTotp(); // timestamp UtcNow
    //var code = totp.ComputeTotp(DateTime.Now);
    //var code = totp.ComputeTotp(DateTime.UnixEpoch);
    var seconds = totp.RemainingSeconds();
    return $"{code} | {DateTime.Now} | Remaining time: {seconds} seg. | Expired On: {DateTime.Now.AddSeconds(seconds)}";
}

bool VerifyTOTPStrict(string inputCode)
{
    bool valid = VerifyBaseTOTP(inputCode, out long timeStepMatched);
    Console.ForegroundColor = ConsoleColor.Blue;
    Console.WriteLine($"*** {nameof(timeStepMatched)}: {timeStepMatched} ***");
    Console.ResetColor();
    valid &= !usedTimeSteps.Contains(timeStepMatched);
    usedTimeSteps.Add(timeStepMatched);

    return valid;
}

bool VerifyBaseTOTP(string inputCode, out long timeStepMatched)
{
    var secret = Base32Encoding.ToBytes(base32Secret);
    var totp = new Totp(secret, step);
    //bool valid = totp.VerifyTotp(inputCode, out timeStepMatched, VerificationWindow.RfcSpecifiedNetworkDelay); //timestamp UtcNow 
    //bool valid = totp.VerifyTotp(DateTime.Now, inputCode, out timeStepMatched, VerificationWindow.RfcSpecifiedNetworkDelay);
    //bool valid = totp.VerifyTotp(DateTime.Now, inputCode, out timeStepMatched); // without delay
    bool valid = totp.VerifyTotp(inputCode, out timeStepMatched);
    //bool valid = totp.VerifyTotp(DateTime.UnixEpoch, inputCode, out timeStepMatched);
    return valid;
}