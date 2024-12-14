import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { RegisterDto } from 'src/app/models/Auth/RegisterDto';
import { LoginDto } from 'src/app/models/Auth/LoginDto';
import { AuthService } from 'src/app/services/auth.service';

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.css'],
})
export class RegisterComponent {
  registerDto: RegisterDto = { email: '', password: '' };
  errorMessage: string = '';

  constructor(private authService: AuthService, private router: Router) {}

  register() {
    this.authService.register(this.registerDto).subscribe({
      next: (response) => {
        const loginDto: LoginDto = {
          email: this.registerDto.email,
          password: this.registerDto.password,
        };

        this.authService.login(loginDto).subscribe({
          next: (authResponse) => {
            this.authService.saveToken(authResponse.token);
            this.router.navigate(['/fundraisers']);
          },
          error: (err) => {
            this.errorMessage = 'Login failed after registration. Please try logging in manually.';
            console.error(err);
          },
        });
      },
      error: (err) => {
        this.errorMessage = 'Registration failed. Please try again.';
        console.error(err);
      },
    });
  }
}
