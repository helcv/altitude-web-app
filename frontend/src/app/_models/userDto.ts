export interface UserDto {
    id: string;
    name: string;
    lastName: string;
    email: string;
    profilePhotoUrl: string;
    dateOfBirth: Date;
    isTwoFactorEnabled: boolean
}