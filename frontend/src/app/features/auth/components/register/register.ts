import { Component, inject } from '@angular/core';
import { Router, RouterLink } from '@angular/router';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { AuthService } from '../../services/auth.service';

@Component({
  selector: 'app-register',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, RouterLink],
  templateUrl: './register.html',
  styleUrl: './register.scss'
})
export class RegisterComponent {
  registerForm: FormGroup;
  authSerivce = inject(AuthService);
  router = inject (Router);
  fb = inject(FormBuilder);
  errorMessage: string | null = null;

  constructor() {
    this.registerForm = this.fb.group({
      userName: ['', Validators.required],
      email: ['', [Validators.required, Validators.email]],
      password: ['', Validators.required],
      confirmPassword: ['', Validators.required]
    });
  }

  onSubmit(): void {
    if (this.registerForm.valid) {
      this.errorMessage = null;
      this.authSerivce.register(this.registerForm.value).subscribe({
        next: (response) => {
          console.log('Login bem-sucedido!', response);
          this.router.navigate(['/recipes']);
        },
        error: (err) => {
          console.error('Falha no login', err);
          this.errorMessage = 'Email ou senha inválidos. Por favor, tente novamente.';
        }
      });
    }
  }
}
