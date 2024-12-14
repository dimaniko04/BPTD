import { Component } from "@angular/core";
import { Router } from "@angular/router";
import { LoginDto } from "src/app/models/Auth/LoginDto";
import { AuthService } from "src/app/services/auth.service";

@Component({
  selector: "app-login",
  templateUrl: "./login.component.html",
  styleUrls: ["./login.component.css"],
})
export class LoginComponent {
  loginDto: LoginDto = { email: "", password: "" };
  errorMessage: string = "";

  constructor(private authService: AuthService, private router: Router) {}

  login(): void {
    this.authService.login(this.loginDto).subscribe({
      next: (response) => {
        this.authService.saveToken(response.token);
        this.router.navigate(["/fundraisers"]);
      },
      error: (err) => {
        this.errorMessage = "Login failed. Please check your credentials.";
        console.error(err);
      },
    });
  }
}
