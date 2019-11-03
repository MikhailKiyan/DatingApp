import { Component, OnInit, Input } from '@angular/core';

import { User } from '../../_models/user';
import { UserService } from './../../_services/user.service';
import { AlertifyService } from './../../_services/alertify.service';
import { AuthService } from './../../_services/auth.service';

@Component({
	selector: 'app-member-card',
	templateUrl: './member-card.component.html',
	styleUrls: ['./member-card.component.css']
})
export class MemberCardComponent implements OnInit {
	@Input() user: User;

	constructor(
			private authService: AuthService,
			private userService: UserService,
			private alertify: AlertifyService) { }

	ngOnInit() { }

	sendLike(recepientUserId: number) {
		const currentUserId = this.authService.decodedToken.nameid;
		this.userService
			.sendLike(currentUserId, recepientUserId)
			.subscribe(
				data => {
					this.alertify.success('You have liked: ' + this.user.knownAs);
				},
				error => {
					this.alertify.error(error);
				}
			);
	}
}
