import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { LoginDto } from 'src/app/models/Auth/LoginDto';
import { RegisterDto } from 'src/app/models/Auth/RegisterDto';
import { AuthService } from 'src/app/services/auth.service';

@Component({
  selector: 'app-auth',
  templateUrl: './auth.component.html',
  styleUrls: ['./auth.component.css']
})
export class AuthComponent {
  isLoginMode = true;
  email = '';
  password = '';

  constructor(private authService: AuthService, private router: Router) {}

  toggleMode(): void {
    this.isLoginMode = !this.isLoginMode;
  }

  submit(): void {
    const authDto: LoginDto | RegisterDto = { email: this.email, password: this.password };

    if (this.isLoginMode) {
      this.authService.login(authDto as LoginDto).subscribe(response => {
        this.authService.saveToken(response.token);
        this.router.navigate(['/fundraisers']);
      });
    } else {
      this.authService.register(authDto as RegisterDto).subscribe(response => {
        this.authService.saveToken(response.token);
        this.router.navigate(['/fundraisers']);
      });
    }
  }
}
