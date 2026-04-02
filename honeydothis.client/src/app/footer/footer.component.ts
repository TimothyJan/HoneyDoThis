import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ThemeType } from '../models/themeType.model';
import { ThemeService } from '../services/theme-service';

@Component({
  selector: 'app-footer',
  templateUrl: './footer.component.html',
  styleUrl: './footer.component.css'
})
export class FooterComponent implements OnInit {
  currentTheme: ThemeType = 'standard';

  constructor(private themeService: ThemeService) {}

  ngOnInit(): void {
    // Subscribe to theme changes
    this.themeService.theme$.subscribe(theme => {
      this.currentTheme = theme;
    });
  }
}
