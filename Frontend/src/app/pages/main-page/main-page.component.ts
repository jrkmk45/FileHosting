import { Component, OnInit } from '@angular/core';
import { MatSelectChange } from '@angular/material/select';
import { ActivatedRoute, Route } from '@angular/router';
import { IFile } from 'src/app/models/file';
import { AuthService } from 'src/app/services/auth.service';
import { FilesService } from 'src/app/services/files.service';
import { SnackbarService } from 'src/app/services/snackbar.service';
import { UserService } from 'src/app/services/user.service';

@Component({
  selector: 'app-main-page',
  templateUrl: './main-page.component.html',
  styleUrls: ['./main-page.component.css']
})
export class MainPageComponent implements OnInit {

  isLoading = true;
  files!: IFile[];

  userId?: number;
  searchTerm?: string;

  accessFilter?: number;

  constructor(private userService: UserService,
    private filesService: FilesService,
    private snackbar: SnackbarService,) {}

  ngOnInit(): void {
    this.userId = this.userService.getUserTokenData()!.id;
    this.getUserFiles(this.userId!);
  }

  onAccessFilterChange(event: MatSelectChange) {
    if (!event.value) {
      this.getUserFiles(this.userId!);
      return;
    }
    this.filesService.queryUserFiles(this.userId!, event.value).subscribe({
      next: (files) => {
        this.files = files;
      },
      error: () => {
        this.snackbar.showMessage("Виникла помилка при завантаженні файлів");
      }
    });
  }
  
  onSearchInputChange(event : any) {
    this.filesService.queryUserFiles(this.userId!, this.accessFilter, event).subscribe({
      next: (files) => {
        this.files = files;
      },
      error: () => {
        this.snackbar.showMessage("Виникла помилка при пошуку файлів");
      }
    });
  }

  onFileUploaded(event : IFile) {
    this.files?.unshift(event);
  }

  onFilesDeleted(event : IFile[]) {
    this.files = this.files.filter(f => !event.includes(f));
  }

  onSearchPerformed(event : IFile[]) {
    this.files = event;
  }

  getUserFiles(userId: number) {
    this.filesService.getUserFilesMetadata(userId).subscribe({
      next: (response) => {
        this.files = response;
        this.isLoading = false;
      },
      error: () => {
        this.snackbar.showMessage("Виникла помилка при завантаженні файлів");
      }
    });
  }
}
