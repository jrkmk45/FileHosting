import { Component, ElementRef, ViewChild } from '@angular/core';
import { SnackbarService } from 'src/app/services/snackbar.service';
import { UserService } from 'src/app/services/user.service';

@Component({
  selector: 'app-user-settings-page',
  templateUrl: './user-settings-page.component.html',
  styleUrls: ['./user-settings-page.component.css']
})
export class UserSettingsPageComponent {
  @ViewChild('fileInput') fileInput!: ElementRef;
  
  selectedFile? : File;

  constructor (public userService: UserService,
      private snackbar: SnackbarService) {
   }

   onImageSelect(event : any) {
    const file = (event.target as HTMLInputElement).files?.[0];
    this.selectedFile = file;
   }

   onOpenImageSelecting() {
    this.fileInput.nativeElement.click();
   }

   onUploadSubmit() {
    if (this.selectedFile == null) {
      this.snackbar.showMessage("Оберіть зображення!");
    }

    const formData = new FormData();
    formData.append('ProfilePicture', this.selectedFile!);
    this.userService.updateUser(formData).subscribe({
      next: () => {
        this.userService.getMyInfo().subscribe();
        this.snackbar.showMessage("Аватар успішно оновлений");
      },
      error: () => {
        this.snackbar.showMessage("Сталась помилка при оновленні аватару");
      }
    });
   }
}
