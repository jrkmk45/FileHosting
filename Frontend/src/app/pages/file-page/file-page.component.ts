import { HttpErrorResponse } from '@angular/common/http';
import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { IFile } from 'src/app/models/file';
import { FilesService } from 'src/app/services/files.service';
import { SnackbarService } from 'src/app/services/snackbar.service';

@Component({
  selector: 'app-file-page',
  templateUrl: './file-page.component.html',
  styleUrls: ['./file-page.component.css']
})
export class FilePageComponent implements OnInit {

  file? : IFile;
  isLoading = true;

  constructor(private filesService: FilesService,
    private route: ActivatedRoute,
    private snackbar: SnackbarService) {}

  ngOnInit() {
    // this.filesService.
    let fileId = this.route.snapshot.paramMap.get('id');
    this.filesService.getFileMetadata(fileId!).subscribe({
      next: (file) => {
        this.file = file;
      },
      error: (error: HttpErrorResponse) => {
        if (error.status == 401) {
          this.snackbar.showMessage("Авторизуйтесь щоб отримати доступ до цього файлу!");
        }
        else if (error.status == 403) {
          this.snackbar.showMessage("У вас немає доступу до цього файлу!");
        }
      }
    })
  }
}
