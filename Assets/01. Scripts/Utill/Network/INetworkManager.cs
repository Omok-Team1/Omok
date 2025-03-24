public interface INetworkManager
{
    void RequestLogIn(SignInData signInData);
    
    void RequestSignUp(SignUpData signUpData);
}