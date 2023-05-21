import { Component } from '@angular/core';
import { FormBuilder, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { AuthService } from 'src/app/services/auth.service';
import { SnackbarService } from 'src/app/services/snackbar.service';
import { passwordStrengthValidator } from 'src/app/validators/passwordStrengthValidator';
import { passwordsMatchValidator } from 'src/app/validators/passwordsMatchValidator';

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.css']
})
export class RegisterComponent {
  hide = true;
  submitPressed = false;
  registerErrorMessages = null;

  constructor(private builder: FormBuilder,
    private authService: AuthService, 
    private router: Router,
    private snackbarService: SnackbarService) {

  }

  registerForm = this.builder.group({
    userName: this.builder.control('', Validators.required),
    password: this.builder.control('', Validators.compose([Validators.required, Validators.minLength(5), passwordStrengthValidator()])),
    confirmPassword: this.builder.control('', Validators.required)
  },
  {
    validators: passwordsMatchValidator()
  }
  );

  get confirmPassword() {
    return this.registerForm.get('confirmPassword');
  }

  get userName() {
    return this.registerForm.get('userName');
  }

  get password() {
    return this.registerForm.get('password');
  }


  proceedRegistration() {
    if (this.registerForm.valid) {
      this.authService.register(this.registerForm.value).subscribe({
        next: () => {
          this.router.navigate(['login']);
          this.snackbarService.showMessage("Successfully registered in!");
        },
        error: (response) => {
          console.error('Error:', response);
          this.registerErrorMessages = response.error.errors;
        }
      });
    }
  }
}
