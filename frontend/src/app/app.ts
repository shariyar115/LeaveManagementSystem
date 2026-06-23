import { Component, OnInit, inject } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { RouterLink, RouterLinkActive, RouterOutlet } from '@angular/router';
import { SelectModule } from 'primeng/select';
import { ToastModule } from 'primeng/toast';
import { AppStateService } from './core/services/app-state.service';

interface NavItem {
  label: string;
  path: string;
  icon: string;
}

@Component({
  selector: 'app-root',
  imports: [RouterOutlet, RouterLink, RouterLinkActive, FormsModule, SelectModule, ToastModule],
  templateUrl: './app.html',
  styleUrl: './app.scss',
})
export class App implements OnInit {
  protected readonly state = inject(AppStateService);

  protected readonly nav: NavItem[] = [
    { label: 'Dashboard', path: '/dashboard', icon: 'pi pi-th-large' },
    { label: 'Apply', path: '/apply', icon: 'pi pi-send' },
    { label: 'Approvals', path: '/approvals', icon: 'pi pi-check-square' },
    { label: 'Leave Types', path: '/leave-types', icon: 'pi pi-tags' },
    { label: 'Settlements', path: '/settlements', icon: 'pi pi-sliders-h' },
  ];

  ngOnInit(): void {
    this.state.loadEmployees();
  }
}
