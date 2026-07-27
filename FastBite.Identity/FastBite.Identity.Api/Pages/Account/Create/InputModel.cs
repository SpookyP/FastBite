// Copyright (c) Duende Software. All rights reserved.
// See LICENSE in the project root for license information.

using System.ComponentModel.DataAnnotations;

namespace FastBite.Pages.Create;

public class InputModel
{
    [Required]
    public string Username { get; set; }

    [Required]
    public string Password { get; set; }

    [Required]
    [Compare("Password")]
    public string ConfirmPassword { get; set; }
    
    [Required]
    public string Name { get; set; }
    
    [Required]
    [EmailAddress]
    public string Email { get; set; }
    [Required]
    [EmailAddress]
    [Compare("Email")]
    public string ConfirmEmail { get; set; }

    public string? ReturnUrl { get; set; }

    public string? Button { get; set; }
}