import { AfterViewInit, Component, ElementRef, HostListener, Input, OnInit, Renderer2, ViewChild } from '@angular/core';
import { hexToCSSFilter } from 'hex-to-css-filter';

@Component({
  selector: 'app-file-icon',
  templateUrl: './file-icon.component.html',
  styleUrls: ['./file-icon.component.css']
})
export class FileIconComponent {

  filterValue?: string;

  @Input() extension! : string;
  @Input() showExtension = true;
  
  backgroundColorFilter? : string;


  ngOnInit() {
    switch (this.extension) {
      case '.exe':
        this.backgroundColorFilter = hexToCSSFilter('#0e63ab').filter;
        break;
      case '.pptx':
        this.backgroundColorFilter = hexToCSSFilter('#ac3a1f').filter;
        break;
      case '.docx':
        this.backgroundColorFilter = hexToCSSFilter('#2980b9').filter;
        break;
      case '.pdf':
        this.backgroundColorFilter = hexToCSSFilter('#db1a1c').filter;
        break;
    }
    this.backgroundColorFilter = this.backgroundColorFilter?.slice(0, -1);
  }
} 
