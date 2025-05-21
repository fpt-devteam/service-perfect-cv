using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using ServicePerfectCV.Application.DTOs.Item.Requests;
using ServicePerfectCV.Application.Services;

namespace ServicePerfectCV.WebApi.Controllers
{
    [ApiController]
    [Route("api/items")]
    public class ItemController : ControllerBase
    {
        private readonly ItemService _itemService;

        public ItemController(ItemService itemService)
        {
            _itemService = itemService;
        }
        [HttpPut("{itemId}")]
        public async Task<IActionResult> UpdateAsync(Guid itemId, [FromBody] ItemUpdateRequest request)
        {

            await _itemService.UpdateItemAsync(itemId, request);
            return Ok();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAsync(Guid id)
        {
            await _itemService.DeleteItemAsync(id);
            return NoContent();
        }



    }
}
