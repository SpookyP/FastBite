using MenuCatalog.Application.DTOs;
using MenuCatalog.Application.IService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MenuCatalog.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MenusController(IMenuService menuService) : ControllerBase
    {
        private readonly IMenuService _menuService = menuService;


        [HttpGet("ObterPorId")]
        [AllowAnonymous]
        public async Task<IActionResult> ObterPorId(int id)
        {
            if (id <= 0)
            {
                return BadRequest("ID inválido. O ID deve ser maior do que zero.");
            }

            var menuId = await _menuService.ObterPorIdAsync(id);

            if (menuId == null)
            {
                return NotFound("Menu não encontrado.");
            }

            return Ok(menuId);

        }

        [HttpGet("ObterTodos")]
        [AllowAnonymous]
        public async Task<IActionResult> ObterTodos()
        {
            var menus = await _menuService.ObterTodosAsync();

            return Ok(menus);
        }

        [HttpPost]
        public async Task<IActionResult> Adicionar([FromBody] MenuCreateEditDto menuCreateDto) //FromBody -> Lê o JSON que está dentro da "caixa" (O corpo do pedido HTTP) ex: { "nome": "Bife", "preco": 15.5 }
        {

            var menuCriado = await _menuService.AdicionarMenuAsync(menuCreateDto);

            return CreatedAtAction(nameof(ObterPorId), new { id = menuCriado.Id }, menuCriado);
        }

        [HttpPut("Atualizar")]
        public async Task<IActionResult> Atualizar(int id, [FromBody] MenuCreateEditDto menuUpdateDto)
        {
            if (id <= 0)
            {
                return BadRequest("ID inválido.");
            }

            try
            {
                await _menuService.AtualizarMenuAsync(id, menuUpdateDto);

                return NoContent();
            }
            catch (KeyNotFoundException ex) 
            {
                return NotFound(ex.Message);
            }
        }

        [HttpDelete("Eliminar")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> RemoverMenu(int id)
        {
            if (id <= 0)
            {
                return BadRequest("ID inválido. O ID deve ser maior do que zero.");
            }

            try
            {
                await _menuService.RemoverMenuAsync(id);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            } 
        }

        [HttpGet("VerDisponibilidade")]
        [AllowAnonymous]
        public async Task<IActionResult> VerDisponibilidade(int id, [FromQuery] int quantidade) //FromQuery - 'ensina' o controller a ler tudo o que vem depois do ? no URL (?quantidade=5)
        {
            if (id <= 0 || quantidade <= 0)
            {
                return BadRequest("O ID do prato e a quantidade devem ser maiores que zero.");
            }

            var qntDisponivel = await _menuService.VerDisponibilidadeAsync(id, quantidade);

            return Ok(qntDisponivel);
        }

        [HttpGet("Disponiveis")]
        [AllowAnonymous]
        public async Task<IActionResult> ObterPratosDisponiveis()
        {
            var menusDisponiveis = await _menuService.ObterPratosDisponiveisAsync();

            return Ok(menusDisponiveis);
        }
    }
}
