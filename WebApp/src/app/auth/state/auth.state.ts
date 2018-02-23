export interface AuthState {
    inProgress: boolean;
    username: string;
    loggedIn: boolean;
    loginError: string;
    signupError: string;
    acquirePasswordResetError: string;
    resetPasswordError: string;
}
