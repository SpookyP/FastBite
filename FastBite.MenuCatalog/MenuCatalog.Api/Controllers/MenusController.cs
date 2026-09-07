using MenuCatalog.Application.DTOs;
using MenuCatalog.Application.IService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MenuCatalog.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MenusController(IItemService menuService, IMenuComboService menuComboService) : ControllerBase
    {

        private readonly IItemService _menuService = menuService;
        private readonly IMenuComboService _menuComboService = menuComboService;

        /// <summary>
        /// Este endpoint permite obter um item do menu pelo seu ID.
        /// </summary>
        /// <param name="id">O ID do item do menu a ser obtido.</param>
        /// <returns>Retorna o item do menu correspondente ao ID fornecido, ou um erro se o ID for inválido ou o item não for encontrado.</returns>
        [HttpGet("ObterPorId")]
        [Authorize(Roles = "Admin,Client")]

        public async Task<IActionResult> ObterPorId(int id)
        {
            if (id <= 0)
            {
                return BadRequest("ID inválido. O ID deve ser maior do que zero.");
            }

            var menuId = await _menuService.ObterPorIdAsync(id);

            if (menuId == null)
            {
                return NotFound("Item não encontrado.");
            }

            return Ok(menuId);

        }
        /// <summary>
        /// Este endpoint permite obter todos os itens do menu.
        /// </summary>
        /// <returns>Retorna uma lista de todos os itens do menu.</returns>
        [HttpGet("ObterTodos")]
        [Authorize(Roles = "Admin,Client")]
        public async Task<IActionResult> ObterTodos()
        {
            var menus = await _menuService.ObterTodosAsync();

            return Ok(menus);
        }

        /// <summary>
        /// Este endpoint permite adicionar um novo item ao menu.
        /// </summary>
        /// <param name="menuCreateDto">O DTO contendo as informações do item do menu a ser criado.</param>
        /// <returns>Retorna o item do menu criado com o respetivo id.</returns>
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Adicionar([FromBody] ItemCreateEditDto menuCreateDto) //FromBody -> Lê o JSON que está dentro da "caixa" (O corpo do pedido HTTP)
        {

            var menuCriado = await _menuService.AdicionarMenuAsync(menuCreateDto);

            return CreatedAtAction(nameof(ObterPorId), new { id = menuCriado.Id }, menuCriado);
        }

        /// <summary>
        /// Este endpoint permite atualizar um item do menu existente pelo seu ID.
        /// </summary>
        /// <param name="id">O ID do item do menu a ser atualizado.</param>
        /// <param name="menuUpdateDto">O DTO contendo as informações do item do menu a ser atualizado.</param>
        /// <returns>Retorna NoContent se a atualização for bem-sucedido e BadRequest se o ID for inválido.</returns>
        [HttpPut("Atualizar")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Atualizar(int id, [FromBody] ItemCreateEditDto menuUpdateDto)
        {
            if (id <= 0)
            {
                return BadRequest("ID inválido.");
            }

            await _menuService.AtualizarMenuAsync(id, menuUpdateDto);

            return NoContent();

        }

        /// <summary>
        /// Este endpoint permite remover um item do menu pelo seu ID.
        /// </summary>
        /// <param name="id">O ID do item do menu a ser removido.</param>
        /// <returns>Retorna NoContent se a remoção for bem-sucedida e BadRequest se o ID for inválido.</returns>
        [HttpDelete("Eliminar")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> RemoverMenu(int id)
        {
            if (id <= 0)
            {
                return BadRequest("ID inválido. O ID deve ser maior do que zero.");
            }


            await _menuService.RemoverMenuAsync(id);

            return NoContent();
        }

        /// <summary>
        /// Este endpoint permite verificar a disponibilidade de um item do menu com base no seu ID e na quantidade desejada.
        /// </summary>
        /// <param name="id">O ID do item do menu a ser verificado.</param>
        /// <param name="quantidade">A quantidade desejada do item do menu.</param>
        /// <returns>Retorna Ok com a quantidade disponível se bem-sucedido e BadRequest se o ID ou a quantidade forem inválidos.</returns>
        [HttpGet("VerDisponibilidade")]
        [AllowAnonymous]
        public async Task<IActionResult> VerDisponibilidade(int id, [FromQuery] int quantidade) //FromQuery - 'ensina' o controller a ler tudo o que vem depois do ? no URL (?quantidade=5)
        {
            if (id <= 0 || quantidade <= 0)
            {
                return BadRequest("O ID do item e a quantidade devem ser maiores que zero.");
            }

            var qntDisponivel = await _menuService.VerDisponibilidadeAsync(id, quantidade);

            return Ok(qntDisponivel);
        }

        /// <summary>
        /// Este endpoint permite obter todos os pratos disponíveis no menu.
        /// </summary>
        /// <returns>Retorna Ok com a lista de pratos disponíveis.</returns>
        [HttpGet("Disponiveis")]
        [AllowAnonymous]
        public async Task<IActionResult> ObterPratosDisponiveis()
        {
            var menusDisponiveis = await _menuService.ObterPratosDisponiveisAsync();

            return Ok(menusDisponiveis);
        }

        /// <summary>
        /// Este endpoint permite montar um combo de menu com base nos IDs do prato, acompanhamento e bebida fornecidos.
        /// </summary>
        /// <param name="request">O DTO contendo os IDs do prato, acompanhamento e bebida para montar o combo.</param>
        /// <returns>Retorna Ok com o combo montado.</returns>
        [HttpPost("combo")]
        [Authorize(Roles = "Admin,Client")]
        public async Task<ActionResult<MenuComboResponseDto>> MontarCombo([FromBody] MenuComboCreateDto request)
        {
            var resultado = await _menuComboService.MontarComboAsync(request);
            return Ok(resultado);
        }

        [HttpPost("descontar-stock")]
        [AllowAnonymous]
        public async Task<IActionResult> DescontarItemsVendidos([FromBody] List<ItemRequestDto> itensVendidos)
        {
            await _menuService.RegistarVendasAsync(itensVendidos);
            return Ok();
        }
    }
}
