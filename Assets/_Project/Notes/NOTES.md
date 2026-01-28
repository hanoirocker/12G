NOVEDADES:
- Se solucionaron problemas de fuentes ocupadas del tipo UI.
- Se modificaron los eventos de Disclaimer e Information.
NOTA: BORRAR TODO LO RELACIONADO AL CHECKPOINT DE RED HOUR AL TERMINAR DE TRABAJAR EN EL EVENTO.

TOCHECK:
- HEADACHES: actualmente se terminó el final de la primera parte, pero queda aún se podría mencionar al inicio de Headaches el dolor de cabeza que le ocasiona el walkie talkie y los aparatos electrónicos. Quizas luego de eso podriamos prender y apagar aparatos aleatoriamente? (microondas, radio, luces de la casa)
- Agregar mas sonidos stereo para los perfiles sonoros de ambiente?

TOFIX:
- Si habilitamos el Walkie Talkie mediante un checkpoint asset, se rompe al mostrarlo o intentar cambiar el canal debido a que no genera una estructura de canales --> Esto sucede cuando no existe extructura de audios de canales para el evento en particular.

TODO - próximo:
- Al iniciar el evento "Red Hour" los cambios cambian de golpe antes de empezar la lógica de glowing, pudiendo ser observado este cambio por el jugador. Buscar la forma de que esto no suceda (Quizas agregar algo al evento al incio, antes del glowing?)
- Componer audio para ScaredReactionLong // LISTO PERO NO FINAL
- Actualmente el sistema de audio por perfiles de ambiente funciona en base a la escena. Por ende, si quisieramos detener la lluvia en algun momento, la lluvia estereo del los perfiles seguiria sonando. Analizar si es conveniente o no ampliar la estructura logica.

TODO - despues:

- Walkie Talkie: Agregar ruidos de estática distintos para cada canal (En todos los assets de WalkieTalkieData).
- Quizás trabajar un poco mas sobre el audio de la alucinación de fernandez, agregar risas femeninas al audio del cuadro del corazón.
- Agregar sonidos para cuando come la pizza.
- Agregar sonidos al tomar bolsas de basura.
- Agregar sonido de mareo y caida del jugador al suelo, dependiendo del lugar donde caiga debe sonar distinto (3 materiales: carpet, wood, mosaic)
- Agregar animación de primera activación de linterna para Unwired? (MAYBE)
- Componer distintos audios según intensidad de Electric Feel. La idea es que sean clips conformados por el sonido actual (loopeable), luego un clip que incluya el layer anterior mas un fade in del nuevo layer de audio (Play one shot), y finalmente el nuevo audio (loopeable).
- Agregar sonido para animación "Night - Stand Up".
- Agregar sonido para animación "Night - Unwired - Look For Flashlight" de FlashlightVC

- Agregar cajas bajo la escalera, o algo.

- Actualizar textos en puertas principales y objetos contemplables al final de la escena de Evening. Por ahora se han dejado de trabajar los SO's.
- Implementar cambios de Video del Settings Menu
- Mostrar Settings Menu Canvas desde el Pause Menu Canvas. Esto implica alterar la lógica del Settings Menu Canvas ya que actualmente al retornar desde cualquier botón "Return" muestra el Main Menu Canvas.

- Eliminar evento de gatito. Ahora se abre la puerta del garage luego de salir de la piecita de la cocina y es el momento de salir de la casa por el garage.

- Something Else: pasillo de la escuela.
  1) Si el jugador luego de "X" cantidad de loops cuando empieza a crecer la luz roja y se acerca el enemigo decide quedarse quieto (jumpscare, bla).
  2) Si corre hasta el final --> animación de que se salva.
- Post Evening Scene (Nueva)? - Se levanta, escucha ruidos de Micaela que le indican que la está pasando mal y sale de la casa (sol naranja que lo destella (Cinematica a partir de un punto en la entrada de la casa)). FIN DEL JUEGO --> Creditos.
