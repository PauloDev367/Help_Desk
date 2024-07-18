using Domain.DomainExceptions;
using Domain.Entities;

namespace DomainTests;
public class CommentTest
{
    [Test]
    public void ShouldThrowAnInvalidCommentExceptionIfClientIsNotDefinedAndItsAClientComment()
    {
        //InvalidCommentException
        var error = Assert.Throws<InvalidCommentException>(() =>
         {
             var comment = new Comment
             {
                 IsClientComment = true,
                 Text = "Mensagem",
             };
         });

        Assert.AreEqual(error.Message, "Client must be specified for client comments.");
    }
}
