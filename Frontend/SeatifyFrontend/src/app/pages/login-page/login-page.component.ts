import { Component } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { AuthService } from '../../services/auth.service';

@Component({
  selector: 'app-login-page',
  standalone: false,
  templateUrl: './login-page.component.html',
  styleUrl: './login-page.component.sass'
})
export class LoginPageComponent {
  loginForm: FormGroup
  isSubmitting = false
  errorMessage = ''

  constructor(
    private fb: FormBuilder,
    private authService: AuthService,
    private router: Router,
    private route: ActivatedRoute
  ) {
    this.loginForm = this.fb.group({
      email: ['', [Validators.required, Validators.email]],
      password: ['', [Validators.required]]
    })
  }

  onSubmit(): void {
    if (this.loginForm.invalid) {
      this.loginForm.markAllAsTouched()
      return
    }

    this.isSubmitting = true
    this.errorMessage = ''

    this.authService.login(this.loginForm.getRawValue()).subscribe({
      next: () => {
        this.isSubmitting = false
        this.router.navigateByUrl(this.getReturnUrl())
      },
      error: (err: Error) => {
        this.isSubmitting = false
        this.errorMessage = err.message
      }
    });
  }

  onDevLogin(): void {
    this.isSubmitting = true
    this.errorMessage = ''

    this.authService.loginAsDev().subscribe({
      next: () => {
        this.isSubmitting = false
        this.router.navigateByUrl(this.getReturnUrl())
      },
      error: (err: Error) => {
        this.isSubmitting = false
        this.errorMessage = err.message
      }
    })
  }

  get f() {
    return this.loginForm.controls
  }

  private getReturnUrl(): string {
    return this.route.snapshot.queryParamMap.get('returnUrl') || '/dashboard'
  }
}
