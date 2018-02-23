import { AbstractControl, ValidationErrors } from "@angular/forms";

export function passwordValidator(control: AbstractControl): ValidationErrors | null {
    const value = control.value as string;

    if (!value || value.length < 8) {
        return {
            passwordValid: "Password must be at least 8 characters long"
        };
    }

    if (!/\d/.test(value)) {
        return {
            passwordValid: "Password must contain at least one digit"
        };
    }

    if (!/[^A-Za-z0-9]/.test(value)) {
        return {
            passwordValid: "Password must contain at least one non letter or digit character"
        };
    }

    return null;
}
