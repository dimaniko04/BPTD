import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { RegisterDto } from 'src/app/models/Auth/RegisterDto';
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
      next: () => this.router.navigate(['/login']),
      error: (err) => {
        this.errorMessage = 'Registration failed. Please try again.';
        console.error(err);
      },
    });
  }
}
