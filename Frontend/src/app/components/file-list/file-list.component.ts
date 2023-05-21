import { Component, Input } from '@angular/core';
import { IFile } from 'src/app/models/file';
import { FilesService } from 'src/app/services/files.service';

@Component({
  selector: 'app-file-list',
  templateUrl: './file-list.component.html',
  styleUrls: ['./file-list.component.css']
})
export class FileListComponent {
  @Input() files?: IFile[];
  @Input() isLoading = false;

  onSelectionUpdated(event: string) {
    this.files!.map(f => {
      f.id == event ? { ...f, id: !f.id } : f;
    })
  }
}
